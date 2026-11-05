<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class UserPetContoroller extends Controller
{
    // 所持アイテム取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');
        $elderUserId = $request->query('elder_user_id');

        $query = DB::table('user_pets')
            ->select(
                'id',
                'elder_user_id',
                'animals_id',
                'nickname',
                'created_at',
                'updated_at'
            );

        if ($id !== null && $id !== '') {
            $item = $query->where('id', $id)->first();

            return response()->json($item);
        }

        if ($elderUserId !== null && $elderUserId !== '') {
            $query->where('elder_user_id', $elderUserId);
        }

        return response()->json($query->get());
    }

    // 所持アイテム登録
    public function store(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'elder_user_id' => 'required|integer|exists:elder_users,id',
            'animals_id' => 'required|integer|exists:user_pets,id',
        ]);

        $id = DB::table('user_pets')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'animals_id' => $validated['animals_id'],
            'nickname' => $request['nickname'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        $item = DB::table('user_pets')
            ->where('id', $id)
            ->first();

        return response()->json($item, 201);
    }
}
