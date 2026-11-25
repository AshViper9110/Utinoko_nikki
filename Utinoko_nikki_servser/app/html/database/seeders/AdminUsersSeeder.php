<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;

class AdminUsersSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('admin_users')->updateOrInsert(
            ['name' => 'jobi'],
            [
                'password' => Hash::make('Yoshidajobi2024'),
                'created_at' => now(),
                'updated_at' => now(),
            ]
        );
    }
}
