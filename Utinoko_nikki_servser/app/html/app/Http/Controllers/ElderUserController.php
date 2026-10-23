<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class ElderUserController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $user = DB::table('elder_users')
                ->select('id', 'name', 'firebase_uid')
                ->where('id', $id)
                ->first();

            return response()->json($user);
        }

        $user = DB::table('elder_users')
            ->select('id', 'name', 'firebase_uid')
            ->get();

        return response()->json($user);
    }
}
